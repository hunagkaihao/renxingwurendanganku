/** 从浏览器设备列表中保留视频输入，排除名称明确标识的内置摄像头。 */
export function externalCameraDevices(devices: MediaDeviceInfo[]): MediaDeviceInfo[] {
    return devices.filter(device => device.kind === 'videoinput' && !!device.deviceId &&
        !/integrated|built[\s-]?in|internal|facetime|内置/i.test(device.label));
}

/** 保留用户明确选择的设备；仅在唯一候选明确标识为 RGB、USB 或外接设备时自动选择。 */
export function selectExternalCamera(devices: MediaDeviceInfo[], selectedId: string): string {
    if (selectedId && devices.some(device => device.deviceId === selectedId)) return selectedId;
    const candidates = devices.filter(device => /rgb|usb|external|外接/i.test(device.label));
    return candidates.length === 1 ? candidates[0].deviceId : '';
}

/** 精确打开已选择的设备，优先使用旧注册页约定的 1280×720；失败时不回退到默认摄像头。 */
export function externalCameraConstraints(deviceId: string): MediaStreamConstraints {
    if (!deviceId) throw new Error('请先选择外接摄像头。');
    return {
        video: { deviceId: { exact: deviceId }, width: { ideal: 1280 }, height: { ideal: 720 } },
        audio: false
    };
}

/** 将浏览器采集或预览异常转换为可操作的提示，不将设备故障混同为识别服务故障。 */
export function terminalCameraError(error: any): string {
    switch (error && error.name) {
        case 'NotAllowedError':
        case 'PermissionDeniedError':
        case 'SecurityError':
            return '摄像头访问被拒绝，请允许此站点使用摄像头，并检查 Windows 摄像头隐私设置。';
        case 'NotFoundError':
        case 'DevicesNotFoundError':
        case 'OverconstrainedError':
            return '所选外接摄像头不可用，请检查 USB 连接，刷新设备列表后重新选择。';
        case 'NotReadableError':
        case 'TrackStartError':
        case 'AbortError':
            return '无法读取所选外接摄像头，请关闭其他占用该设备的软件，并检查连接和驱动后重试。';
        default:
            return '外接摄像头开启或预览失败，请重新选择设备后重试，或切换账号密码登录。';
    }
}
