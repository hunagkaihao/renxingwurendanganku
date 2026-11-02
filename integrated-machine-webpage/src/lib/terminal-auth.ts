import Ajax from './ajax';
import { CancelToken } from 'axios';

/** 终端登录后允许继续的留样、取样页面，避免接受任意跳转地址。 */
export const sampleDestinations = ['stockinstepctn', 'stockinstepctn2', 'stockoutstepctn', 'stockoutstepctn2'];

/** WMS 签发的登录结果，三种登录方式使用同一令牌格式。 */
export interface TerminalLoginResult {
    /** 当前登录用户 GUID。 */
    id: string;
    /** 调用 WMS 业务接口使用的 JWT。 */
    token: string;
}

/** 仅获取本轮账号密码登录结果，调用方确认页面仍有效后再建立会话。 */
export async function passwordLogin(name: string, password: string, cancelToken: CancelToken): Promise<TerminalLoginResult> {
    const config = { cancelToken, terminalLogin: true };
    const response = await Ajax.post('/api/app/account/login', { name, password }, config);
    return response.data;
}

/** 由 WMS 调用配置的识别服务完成身份比对，浏览器只提交方式和当次照片。 */
export async function biometricLogin(method: string, imageData: string, cancelToken: CancelToken): Promise<TerminalLoginResult> {
    // WMS 设备调用最长 120 秒，前端多留 10 秒接收明确的超时结果。
    const config = { cancelToken, timeout: 130000, terminalLogin: true };
    const response = await Ajax.post('/api/app/account/biometric-login', { method, imageData }, config);
    return response.data;
}

/** 优先展示 WMS 的业务错误；设备无法访问时提供可操作的提示。 */
export function terminalLoginError(error: any): string {
    if (error && error.response && error.response.data && error.response.data.error) {
        const message = error.response.data.error.message;
        if (message) return message;
    }
    if (error && error.code === 'ECONNABORTED') return '登录响应超时，请重试或切换登录方式。';
    return error && error.message && !error.response
        ? '登录未完成，请检查服务连接后重试。' : '登录失败，请检查输入或切换登录方式。';
}
