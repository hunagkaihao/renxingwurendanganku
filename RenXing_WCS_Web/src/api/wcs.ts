import { http } from '../services/http';
import type { LogRecord, MjjTag, OrderInfo, PlcTag, ResponseDto, WcsState } from '../types/wcs';

export const getWcsState = () => http.get<WcsState>('/wcs/dispatch/core/wcsStatus').then(r => r.data);
export const changeWcsState = (action: 'pause' | 'restart') => http.post<ResponseDto>(`/wcs/dispatch/core/${action}`).then(r => r.data);
export const runTest = (action: 'start' | 'stop' | 'restart') => http.post<ResponseDto>(`/wcs/test/${action}`).then(r => r.data);
export const getOrders = () => http.get<OrderInfo[]>('/wcs/dispatch/order/unDoneOrders').then(r => r.data);
export const getOrder = (orderCode: string) => http.get<OrderInfo>('/wcs/dispatch/order/oneOrder', { params: { orderCode } }).then(r => r.data);
export const finishOrder = (orderCode: string) => http.post<ResponseDto>('/wcs/dispatch/order/forceDone', { orderCode }).then(r => r.data);
export const cancelOrder = (orderCode: string) => http.post<ResponseDto>('/wcs/dispatch/order/cancelOrder', { orderCode }).then(r => r.data);
export const getPlcTags = () => http.get<PlcTag[]>('/wcs/plc/plcMonitor').then(r => r.data);
export const getMjjTags = () => http.get<MjjTag[]>('/wcs/mjj/mjjStatusOfNmValMapList').then(r => r.data);
export const getLogs = (params: { msgSnip: string; grade: string; logCntMax: number }) => http.get<LogRecord[]>('/wcs/log/query', { params }).then(r => r.data);
