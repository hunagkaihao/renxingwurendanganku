/** 校验业务编号文本，允许前导零，与人脸后端范围一致。 */
export function validateUserId(value) {
  const text = String(value).trim()
  if (!/^\d+$/.test(text) || Number(text) < 1 || Number(text) > 2147483647) {
    throw new Error('编号必须为 1～2147483647 的整数。')
  }
  return text
}

/** 为 Base64 膨胀和 JSON 留出空间，避免超过后端 6 MB 请求限制。 */
export function validateImage(file) {
  if (!['image/jpeg', 'image/png', 'image/bmp', 'image/x-ms-bmp'].includes(file.type)) {
    throw new Error('请选择 JPEG、PNG 或 BMP 图片。')
  }
  if (file.size <= 0 || file.size > 4_000_000) throw new Error('图片大小须大于 0 且不超过 4 MB。')
}

/** 将 HTTP 请求发送到代理或用户指定的 API；不自动重试有副作用的操作。 */
export async function request(base, path, method = 'GET', body) {
  const controller = new AbortController()
  const timer = setTimeout(() => controller.abort(), 240_000)
  try {
    const response = await fetch(`${base.replace(/\/$/, '')}${path}`, {
      method,
      signal: controller.signal,
      ...(body === undefined ? {} : { headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) }),
    })
    const text = await response.text()
    let result = null
    if (text) {
      try { result = JSON.parse(text) }
      catch { throw new Error(`HTTP ${response.status}：响应不是 JSON，请检查 API 地址和代理。`) }
    }
    if (!response.ok) {
      const detail = result?.errors ? Object.values(result.errors).flat().join('；') : result?.detail || result?.title
      throw new Error(`HTTP ${response.status}：${detail || '请求失败，请查看后端日志。'}`)
    }
    return result
  } catch (error) {
    if (controller.signal.aborted) throw new Error('请求超过 240 秒，操作结果未知，请检查后端日志后再决定是否重试。')
    if (error instanceof TypeError) throw new Error('无法连接 API，请检查后端是否启动、代理目标与跨域配置。')
    throw error
  } finally { clearTimeout(timer) }
}

/** 根据公开接口业务结果显示状态，展示注册/识别失败阶段与 SDK 错误码。 */
export function interpretResult(action, data) {
  if ((action === 'veinRegister' || action === 'veinMatch') && typeof data?.success === 'boolean' && typeof data.msg === 'string') {
    const progress = action === 'veinRegister' && Number.isInteger(data.completedSamples) ? `（已采集 ${data.completedSamples}/6）` : ''
    const code = Number.isInteger(data.sdkCode) ? ` SDK 错误码：${data.sdkCode}` : ''
    const identity = action === 'veinMatch' && data.success && Number.isInteger(data.fingerId) ? ` 模板编号：${data.fingerId}` : ''
    return { type: data.success ? 'success' : 'error', message: `${data.msg}${identity}${progress}${code}` }
  }
  if (action === 'faceMatch' || action === 'veinMatch') {
    if (Number.isInteger(data) && data > 0) return { type: 'success', message: `${action === 'faceMatch' ? '匹配用户' : '匹配设备模板'}：${data}` }
    if (data === 0) return { type: 'warning', message: action === 'faceMatch' ? '未匹配：陌生人、空库或候选身份不唯一。' : '未完成识别：等待手指失败，请查看设备日志。' }
    if (data === -1) return { type: 'error', message: action === 'faceMatch' ? '识别失败：图片或识别服务异常，请查看后端日志。' : '指静脉 SDK 比对失败，请查看后端日志。' }
  }
  if (action === 'faceAdd' || action === 'faceDelete') {
    if (data?.errno === 0 || data?.errno === 99) return { type: data.errno === 0 ? 'success' : 'error', message: data.msg || `业务结果码：${data.errno}` }
  }
  if (action === 'veinRegister' && data === null) return { type: 'warning', message: '注册请求已结束，接口不返回注册结果。请在后端日志确认六次采集及模板写入成功。' }
  if (action === 'veinDelete' && typeof data === 'boolean') return { type: data ? 'success' : 'error', message: data ? '设备模板删除成功；后台记录删除结果请查看日志。' : '设备模板删除失败，请查看后端日志。' }
  return { type: 'error', message: '接口响应结构不符合预期，请检查后端版本和 API 地址。' }
}
