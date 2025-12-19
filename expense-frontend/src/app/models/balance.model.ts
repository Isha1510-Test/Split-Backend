export interface UserBalance {
  userId: number;
  userName: string;
  amount: number;
}

export interface Balance {
  userId: number;
  userName: string;
  totalOwed: number;
  totalOwing: number;
  netBalance: number;
  owedBy?: UserBalance[];
  owesTo?: UserBalance[];
}

export interface SimplifiedDebt {
  from: string;
  to: string;
  amount: number;
}