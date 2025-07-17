import type { Team } from "../types/Team";
import { apiGet } from "./axios";

export async function GetTeamsForUser() {
  const response = await apiGet<Team[]>("Teams", true);

  return response;
}
