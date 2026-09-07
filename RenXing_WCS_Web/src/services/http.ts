import axios from 'axios';
import { WCS_API_URL } from '../serviceConfig';

export const http = axios.create({ baseURL: WCS_API_URL, timeout: 5000 });
