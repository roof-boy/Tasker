import type { ApiResponse } from "../types/ApiResponse";
import type { UserLoginResponse } from "../types/User";
import { apiPost } from "./axios";

export default async function LoginUser(data: any) {
  const response: ApiResponse<UserLoginResponse> = await apiPost(
    "Auth/Login",
    JSON.stringify(data),
    { headers: { "Content-Type": "application/json" } },
    true
  );

  return response;
}