import Ajax from './ajax';
import { CancelToken } from 'axios';

/** 管理员可选择的 WMS 人员，ID 是账号 GUID，不能用特征编号代替。 */
export interface RegistrationUser {
    /** WMS 账号 GUID。 */
    id: string;
    /** 登录账号。 */
    userName: string;
    /** 人员姓名，未维护时为空。 */
    name?: string;
    /** 账号是否已启用。 */
    isActive?: boolean;
}

/** 本次指静脉录入的设备进度；设备完成仍需等待原录入请求确认 WMS 绑定成功。 */
export interface RegistrationProgress {
    /** 每次录入独立生成的操作标识，防止串用上一次采集提示。 */
    operationId: string;
    /** 设备实际所处阶段或 WMS 绑定阶段。 */
    stage: string;
    /** 当前阶段应向操作人员展示的指令。 */
    message: string;
    /** 已通过同指校验的采集次数，范围为 0～6。 */
    completedSamples: number;
    /** 完成注册需要的采集总次数，固定为 6。 */
    totalSamples: number;
    /** 设备采集及写入是否已经结束，不代表 WMS 绑定完成。 */
    isComplete: boolean;
    /** 设备最终结果；尚未结束时为空。 */
    success?: boolean | null;
    /** 设备失败时的原始 SDK 错误码。 */
    sdkCode?: number | null;
}

/** 使用浏览器随机源创建 UUID，关联本次设备采集和进度查询。 */
export function createRegistrationOperationId(): string {
    const bytes = window.crypto.getRandomValues(new Uint8Array(16));
    bytes[6] = (bytes[6] & 15) | 64;
    bytes[8] = (bytes[8] & 63) | 128;
    const hex = Array.from(bytes, value => value.toString(16).padStart(2, '0')).join('');
    return hex.slice(0, 8) + '-' + hex.slice(8, 12) + '-' + hex.slice(12, 16) + '-' + hex.slice(16, 20) + '-' + hex.slice(20);
}

/** 只将已登录且具有 admin 角色的会话作为人员注册入口；实际授权仍由后端校验。 */
export function isTerminalAdmin(user: any): boolean {
    return !!user && !!user.id && Array.isArray(user.roles) && user.roles.indexOf('admin') >= 0;
}

/** 特征编号只接受 1～2147483647 的十进制整数，避免截断或溢出后绑定错误人员。 */
export function registrationIdentifier(value: string): number {
    if (!/^[1-9]\d*$/.test(value) || !Number.isSafeInteger(Number(value)) || Number(value) > 2147483647)
        throw new Error('请填写 1～2147483647 的整数特征编号。');
    return Number(value);
}

/** 沿用终端请求自行展示错误的约定，保留 Axios 的请求配置类型兼容性。 */
function requestConfig(cancelToken?: CancelToken, timeout: number = 30000) {
    return { cancelToken, timeout, terminalLogin: true };
}

/** 在建立管理员会话后由服务器确认角色和用户编辑权限。 */
export async function checkRegistrationAccess(cancelToken?: CancelToken): Promise<void> {
    await Ajax.get('/api/terminal-registration/access', requestConfig(cancelToken));
}

/** 分页查找已有 WMS 人员，避免一次加载全部账号。 */
export async function searchRegistrationUsers(filter: string, pageIndex: number, cancelToken: CancelToken): Promise<any> {
    const result = await Ajax.post('/api/terminal-registration/users/search', { filter, pageIndex, pageSize: 20 },
        requestConfig(cancelToken));
    return result.data;
}

/** 获取可分配角色，密码和账号创建规则沿用 WMS。 */
export async function registrationRoles(cancelToken: CancelToken): Promise<any[]> {
    const result = await Ajax.get('/api/terminal-registration/roles', requestConfig(cancelToken));
    return result.data.items || [];
}

/** 创建人员账号；成功后由调用方选中该账号继续录入，不重复创建。 */
export async function createRegistrationUser(input: any, cancelToken: CancelToken): Promise<RegistrationUser> {
    const result = await Ajax.post('/api/terminal-registration/users', input, requestConfig(cancelToken));
    return result.data;
}

/** 读取账号当前的绑定及停用预留记录，供管理员核对录入结果。 */
export async function registrationBindings(userId: string, cancelToken: CancelToken): Promise<any[]> {
    const result = await Ajax.get('/api/Users/' + encodeURIComponent(userId) + '/biometric-bindings',
        requestConfig(cancelToken));
    return result.data;
}

/** 服务端完成录入并启用绑定；指静脉不提交模板编号，由后端分配，六次采集最多等待 240 秒。 */
export async function enrollRegistrationUser(userId: string, method: string, identifier: number | undefined,
    imageData: string, cancelToken: CancelToken, operationId?: string): Promise<any> {
    const result = await Ajax.post('/api/terminal-registration/users/' + encodeURIComponent(userId) + '/enroll',
        { method, identifier, imageData, operationId }, requestConfig(cancelToken, 250000));
    return result.data;
}

/** 通过 WMS 授权查询真实采集提示，不向浏览器开放设备服务地址。 */
export async function registrationProgress(userId: string, operationId: string, cancelToken: CancelToken): Promise<RegistrationProgress> {
    const result = await Ajax.get('/api/terminal-registration/users/' + encodeURIComponent(userId)
        + '/enroll-progress/' + encodeURIComponent(operationId), requestConfig(cancelToken, 8000));
    return result.data;
}

/** 优先显示服务器业务错误；超时时提醒核对状态，避免自动重试不可撤销的设备操作。 */
export function registrationError(error: any): string {
    if (error && error.response) {
        if (error.response.status === 404) return '人员注册接口未找到（404），请更新并重启 WMS 后端，确认已包含人员注册功能。';
        if (error.response.status === 401 || error.response.status === 403) return '管理员会话已失效或权限不足，请返回操作台重新登录。';
        const data = error.response.data;
        if (data && data.error && data.error.message) return data.error.message;
    }
    if (error && error.code === 'ECONNABORTED') return '操作响应超时，请先查询人员或绑定状态、核查设备日志，再决定是否重试。';
    return error && error.message ? error.message : '操作未完成，请检查服务连接。';
}
