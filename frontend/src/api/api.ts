import axios, { AxiosInstance } from 'axios';

const api: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5187/api', 
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    // Adicione outros headers necessários, como tokens de autenticação
  }
});

export default api;