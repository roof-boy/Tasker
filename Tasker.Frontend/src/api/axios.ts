import axios, { AxiosError, type CreateAxiosDefaults } from "axios";
import type { ApiResponse } from "../types/ApiResponse";

const axiosConfig = {
  baseURL: import.meta.env.VITE_API_URL,
} satisfies CreateAxiosDefaults;

// Create Axios Instance
export const axiosInstance = axios.create(axiosConfig);

/**
 * Custom GET method to return objects as ApiResponse types directly
 * @param url 
 * @param config 
 * @returns 
 */
export async function apiGet<T>(url: string, config?: any): Promise<ApiResponse<T>> {
  try {
    const response = await axiosInstance.get<T>(url, config);
    return {
      success: true,
      data: response.data,
    };
  } catch (err) {
    const error = err as AxiosError;

    return {
      success: false,
      error: (error.response?.data as any)?.error ?? error.message,
      data: error.response?.data,
    };
  }
}

/**
 * Custom POST method to post objects and return results as ApiResponse types directly
 * @param url 
 * @param body 
 * @param config 
 * @returns ApiResponse
 */
export async function apiPost<T, B = any>(
  url: string,
  body: B,
  config?: any
): Promise<ApiResponse<T>> {
  try {
    const response = await axiosInstance.post<T>(url, body, config);
    return {
      success: true,
      data: response.data,
    };
  } catch (err) {
  const error = err as AxiosError;

  const responseData = error.response?.data;

  const message =
    typeof responseData === "string"
      ? responseData // directly use the plain string
      : error.message;

  return {
    success: false,
    error: message,
    data: responseData,
  };
}
}
