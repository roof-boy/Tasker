export interface User {
  id: string;
  userName: string;
  email: string;
}

export interface UserLoginResponse extends User {
  roles: string[];
}
