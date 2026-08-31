const signalR = require('@microsoft/signalr')
import { WCS_HUB_URL } from './serviceConfig'

const instance = new signalR.HubConnectionBuilder()
.withUrl(WCS_HUB_URL)
.withAutomaticReconnect()
.build();

instance.start().then(function() {
    console.log("WcsHub已连接");
}).catch(function(err){
    console.log('WcsHub连接失败' + err)
});

export default instance;
