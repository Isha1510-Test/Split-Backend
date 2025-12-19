export interface User {
  id?: number;
  username: string;
  email: string;
  name: string;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  name: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserBalance {
  userId: number;
  userName: string;
  totalOwed: number;
  totalOwing: number;
  netBalance: number;
  groupBalances: GroupBalance[];
}

export interface GroupBalance {
  groupId: number;
  groupName: string;
  netBalance: number;
}