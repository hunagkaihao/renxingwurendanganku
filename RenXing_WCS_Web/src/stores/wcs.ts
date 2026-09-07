import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { getWcsState } from '../api/wcs';
import { hubConnection, startHub } from '../services/signalr';
import type { OrderInfo, WcsState } from '../types/wcs';

export const useWcsStore = defineStore('wcs', () => {
  const state = ref<WcsState>('Unknown');
  const orders = ref<OrderInfo[]>([]);
  const connectionStatus = ref<'connected' | 'reconnecting' | 'disconnected' | 'failed'>('disconnected');
  const connected = computed(() => connectionStatus.value === 'connected');
  let lifecycleBound = false;

  async function initialize() {
    try {
      state.value = await getWcsState();
      if (!lifecycleBound) {
        hubConnection.onreconnecting(() => { connectionStatus.value = 'reconnecting'; });
        hubConnection.onreconnected(() => { connectionStatus.value = 'connected'; });
        hubConnection.onclose(() => { connectionStatus.value = 'disconnected'; });
        lifecycleBound = true;
      }
      connectionStatus.value = 'reconnecting';
      await startHub();
      connectionStatus.value = 'connected';
    } catch (error) {
      connectionStatus.value = 'failed';
      throw error;
    }
    hubConnection.off('UpdateWcsStatus');
    hubConnection.off('UpdateUndoneOrders');
    hubConnection.on('UpdateWcsStatus', value => { state.value = value as WcsState; });
    hubConnection.on('UpdateUndoneOrders', value => { orders.value = value as OrderInfo[]; });
  }
  return { state, orders, connected, connectionStatus, initialize };
});
