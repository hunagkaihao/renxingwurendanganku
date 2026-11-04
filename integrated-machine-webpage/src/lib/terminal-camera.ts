/** 从浏览器设备列表中保留视频输入，排除名称明确标识的内置摄像头。 */
export function externalCameraDevices(devices: MediaDeviceInfo[]): MediaDeviceInfo[] {
    return devices.filter(device => device.kind === 'videoinput' && !!device.deviceId &&
        !/integrated|built[\s-]?in|internal|facetime|内置/i.test(device.label));
}

/** 当前站点的浏览器摄像头权限；unknown 表示浏览器不支持查询，不能据此判定已拒绝。 */
export type CameraPermissionState = PermissionState | 'unknown';

/** 只读查询当前页面所属站点的权限，不打开摄像头，也不修改浏览器设置。 */
export async function readCameraPermissionState(permissions?: Permissions): Promise<CameraPermissionState> {
    if (!permissions || typeof permissions.query !== 'function') return 'unknown';
    try {
        const status = await permissions.query({ name: 'camera' as PermissionName });
        return status.state;
    } catch (_) {
        // 部分浏览器未实现 camera 权限查询，仍允许通过显式设备请求取得授权和准确错误。
        return 'unknown';
    }
}

/** 将当前站点的权限状态转换为用户能直接核对的说明。 */
export function cameraPermissionLabel(state: CameraPermissionState): string {
    return { granted: '已允许', prompt: '尚未授权', denied: '已阻止', unknown: '浏览器未提供权限状态' }[state];
}

/** 浏览器摄像头发现结果，区分未完成授权与已经提供的可选设备。 */
export interface ExternalCameraDiscovery {
    /** 可精确打开的外接候选；同名的双目视频通道分别保留。 */
    devices: MediaDeviceInfo[];
    /** 浏览器尚未提供完整设备信息，需要显示独立授权入口。 */
    needsAuthorization: boolean;
    /** 当前设备列表或授权状态的操作说明。 */
    message: string;
}

/** 不将授权前被隐藏的视频输入误报为 USB 硬件未连接。 */
export function inspectExternalCameras(devices: MediaDeviceInfo[], permission: CameraPermissionState = 'unknown'): ExternalCameraDiscovery {
    const videoInputs = devices.filter(device => device.kind === 'videoinput');
    const candidates = externalCameraDevices(devices);
    const anonymous = videoInputs.some(device => !device.deviceId || !device.label);
    const needsAuthorization = !candidates.length || anonymous || permission === 'denied';
    let message = '';
    if (permission === 'denied') message = '当前页面所在站点的摄像头权限已阻止，请从本页面地址栏打开站点设置并允许摄像头；其他端口或地址的授权不适用于本页面。';
    else if (permission === 'granted' && (anonymous || !videoInputs.length)) message = '当前站点已允许摄像头，但浏览器尚未提供可选设备信息，请点击“初始化并刷新设备”；如失败，请根据下方错误检查 Windows 权限或设备占用。';
    else if (anonymous) message = '浏览器尚未提供完整摄像头信息，请先授权并刷新，再选择外接 RGB 摄像头。';
    else if (!videoInputs.length) message = '浏览器尚未提供视频输入设备，请先授权并刷新；已授权后仍无设备时，请检查 Windows 摄像头权限及视频驱动。';
    else if (!candidates.length) message = '浏览器当前只提供内置摄像头，请授权并刷新，或在浏览器摄像头设置中选择外接 RGB 设备。';
    else if (candidates.length > 1) message = '检测到 ' + candidates.length + ' 路可选视频输入；双目摄像头可能显示多个同名 RGB Camera，请分别选择并查看画面。';
    return { devices: candidates, needsAuthorization, message };
}

/**
 * 仅由用户点击独立授权按钮触发；浏览器可能临时打开默认摄像头，流仅供授权后列举设备。
 * @param mediaDevices 当前浏览器的媒体设备接口。
 * @param isCurrent 判断页面是否仍属于本次操作，防止离开后处理迟到的权限响应。
 * @param holdStream 将临时流交给页面管理，使离开页面时也能立即关闭。
 * @returns 授权后浏览器可见的全部设备；临时流在返回或失败时关闭，不用于预览或拍照。
 */
export async function authorizeCameraDevices(mediaDevices: MediaDevices, isCurrent: () => boolean,
    holdStream: (stream: MediaStream) => void): Promise<MediaDeviceInfo[]> {
    const stream = await mediaDevices.getUserMedia({ video: true, audio: false });
    try {
        if (!isCurrent()) return [];
        holdStream(stream);
        return await mediaDevices.enumerateDevices();
    } finally {
        stream.getTracks().forEach(track => track.stop());
    }
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
