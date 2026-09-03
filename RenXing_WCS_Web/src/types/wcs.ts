export type WcsState = 'Running' | 'Pause' | 'Unknown';

export interface OrderInfo {
  orderCode: string; orderType: string; orderState: string; plateCode: string;
  startNode: string; endNode: string; execStep: string; taskId: number;
  jobs?: OrderJob[];
}
export interface OrderJob { id: number; pathStep: number; nodeName: string; cmdName: string; state: string; execInfo: string; }
export interface PlcTag { monitorTagName: string; monitorTagAddr: string; monitorTagValue: string; }
export interface MjjTag { tagName: string; tagValue: string; }
export interface LogRecord { id: number; date: string; grade: string; source: string; message: string; }
export interface ResponseDto { success: boolean; message: string; }
