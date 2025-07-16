export interface User {
  id: string;
  userName: string;
  email: string;
}

export interface UserLoginResponse extends User {
  tokens: {
    accessToken: string;
  };
  roles: string[];
}
