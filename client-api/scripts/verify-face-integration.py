"""通过 ClientApi 的真实 HTTP 接口验证模型、身份绑定和异常退出清理。"""
import argparse
import base64
import ctypes
import json
import os
import socket
import subprocess
import tempfile
import time
from pathlib import Path

import requests

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--application', type=Path, required=True, help='ClientApi.dll 的绝对路径')
parser.add_argument('--image', type=Path, required=True, help='用于临时库测试的单人正脸 JPEG')
options = parser.parse_args()
application = options.application.resolve()
fixture = options.image.resolve()
image = base64.b64encode(fixture.read_bytes()).decode('ascii')
connection = requests.Session()
connection.trust_env = False

with tempfile.TemporaryDirectory(prefix='client-face-check-') as directory:
    temporary = Path(directory)
    with socket.socket() as sock:
        sock.bind(('127.0.0.1', 0))
        port = sock.getsockname()[1]
    url = f'http://127.0.0.1:{port}'
    log = (temporary / 'application.log').open('w', encoding='utf-8')
    command = ['dotnet', str(application), '--urls', url, '--Vein:Enable', 'false',
               '--FaceRecognition:DataPath', str(temporary / 'identities')]
    process = subprocess.Popen(command, cwd=application.parent, stdout=log, stderr=subprocess.STDOUT)
    child_handle = None
    try:
        deadline = time.monotonic() + 180
        while time.monotonic() < deadline:
            if process.poll() is not None:
                raise RuntimeError('ClientApi exited before readiness')
            try:
                if connection.get(url + '/swagger/index.html', timeout=2).status_code == 200:
                    break
            except requests.RequestException:
                pass
            time.sleep(0.25)
        else:
            raise TimeoutError('ClientApi startup exceeded 180 seconds')

        def post(route, body):
            """调用现有公开人脸接口并校验 HTTP 成功状态。"""
            response = connection.post(url + '/Face/' + route, json=body, timeout=60)
            response.raise_for_status()
            return response.json()

        assert post('FaceMatch', {'ImageData': image}) == 0
        added = post('FaceAdd', {'UserId': '123456', 'ImageData': image})
        assert added['errno'] == 0, added
        assert added['data']['log_id']
        assert post('FaceMatch', {'ImageData': image}) == 123456
        assert post('FaceAdd', {'UserId': '123457', 'ImageData': image})['errno'] == 0
        assert post('FaceMatch', {'ImageData': image}) == 0, 'Ambiguous identities must not match'
        assert post('FaceDelete', {'UserId': '123457'})['errno'] == 0
        assert post('FaceMatch', {'ImageData': image}) == 123456
        assert post('FaceDelete', {'UserId': '123456'})['errno'] == 0
        assert post('FaceMatch', {'ImageData': image}) == 0
        assert post('FaceDelete', {'UserId': '123456'})['errno'] == 99
        assert post('FaceAdd', {'UserId': '0', 'ImageData': image})['errno'] == 99
        assert post('FaceMatch', {'ImageData': 'bad-image'}) == -1
        missing = connection.post(url + '/Face/FaceAdd', json={'ImageData': image}, timeout=5)
        assert missing.status_code == 400
        print('PASS: actual ClientApi HTTP enrollment, SFace matching, ambiguity rejection, deletion, invalid input and legacy response contracts')

        # 仅查询本次启动的子进程 ID，不输出命令行中的内部令牌。
        child_query = f"Get-CimInstance Win32_Process -Filter 'ParentProcessId = {process.pid}' | Where-Object {{$_.Name -eq 'python.exe'}} | Select-Object -ExpandProperty ProcessId"
        child_output = subprocess.check_output(['powershell', '-NoProfile', '-Command', child_query], text=True)
        child_pids = [int(line.strip()) for line in child_output.splitlines() if line.strip().isdigit()]
        assert len(child_pids) == 1, child_pids
        kernel = ctypes.WinDLL('kernel32', use_last_error=True)
        kernel.OpenProcess.argtypes = [ctypes.c_uint32, ctypes.c_bool, ctypes.c_uint32]
        kernel.OpenProcess.restype = ctypes.c_void_p
        kernel.WaitForSingleObject.argtypes = [ctypes.c_void_p, ctypes.c_uint32]
        kernel.CloseHandle.argtypes = [ctypes.c_void_p]
        child_handle = kernel.OpenProcess(0x00100000, False, child_pids[0])
        assert child_handle, 'Could not acquire child process handle'
        process.kill()
        process.wait(timeout=10)
        assert kernel.WaitForSingleObject(child_handle, 10000) == 0, 'Orphan face worker did not exit'
        print('PASS: abruptly stopping ClientApi automatically terminates its face worker')
    except Exception:
        log.flush()
        print((temporary / 'application.log').read_text(encoding='utf-8', errors='replace')[-10000:])
        raise
    finally:
        if process.poll() is None:
            process.kill()
            process.wait(timeout=10)
        if child_handle:
            kernel.CloseHandle(child_handle)
        log.close()
