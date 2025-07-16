import type { User } from "./User";

export interface Team {
  id: string;
  name: string;
  createdBy: User;
  leader: User;
}
