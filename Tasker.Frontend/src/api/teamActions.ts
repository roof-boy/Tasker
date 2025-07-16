import type { Team } from "../types/Team";
import { apiGet } from "./axios";

export async function GetTeamsForUser(token: string) {
  const response = await apiGet<Team[]>("Teams", {
    headers: { Authorization: `Bearer ${token}` },
  });

  return response;
}
