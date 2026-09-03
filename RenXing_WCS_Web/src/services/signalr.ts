import { HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { WCS_HUB_URL } from '../serviceConfig';

export const hubConnection = new HubConnectionBuilder()
  .withUrl(WCS_HUB_URL)
  .withAutomaticReconnect()
  .build();

export async function startHub(): Promise<void> {
  if (hubConnection.state === HubConnectionState.Disconnected) await hubConnection.start();
}
