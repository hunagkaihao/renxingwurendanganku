import axios from 'axios'
import { WCS_API_URL } from './serviceConfig'

const instance = axios.create({
    baseURL: WCS_API_URL,
    timeout:2000
});

export default instance;
