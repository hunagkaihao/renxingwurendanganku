const signalR = require('@microsoft/signalr')

const instance = new signalR.HubConnectionBuilder()
.withUrl("http://localhost:3270/hub")
// .withUrl("http://192.168.0.119:3270/hub") //公司麒麟电脑无线
// .withUrl("http://192.168.1.135:3270/hub") //公司麒麟电脑有线
// .withUrl('http://192.168.0.119:3270/hub') //公司DELL电脑无线
// .withUrl('http://192.168.10.247:3270/hub') //公司DELL电脑有线
 //.withUrl("http://192.168.0.129:3270/ecsHub") //因朵2期
.withAutomaticReconnect()
.build();

instance.start().then(function() {
    console.log("EcsHub已连接");
}).catch(function(err){
    console.log('EcsHub连接失败' + err)
});

export default instance;