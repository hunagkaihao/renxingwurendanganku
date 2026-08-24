import axios from 'axios'

const instance = axios.create({
    baseURL: 'http://localhost:3270',
    // baseURL: 'http://192.168.0.119:3270', //公司麒麟电脑无线
    // baseURL: 'http://192.168.1.135:3270', //公司麒麟电脑有线
    // baseURL: 'http://192.168.0.119:3270', //公司DELL电脑无线
    // baseURL: 'http://192.168.10.247:3270', //公司DELL电脑有线
    //baseURL: 'http://192.168.0.129:3270', //因朵2期
    timeout:2000
});

export default instance;
