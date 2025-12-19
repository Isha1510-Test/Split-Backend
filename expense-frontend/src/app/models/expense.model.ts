export enum SplitType {
  EQUAL = 'EQUAL',
  EXACT = 'EXACT',
  PERCENTAGE = 'PERCENTAGE'
}

export interface ExpenseSplit {
  id?: number;
  userId: number;
  userName?: string;
  amount?: number;
  percentage?: number;
}

export interface Expense {
  id?: number;
  description: string;
  amount: number;
  paidById: number;
  paidByName?: string;
  groupId: number;
  groupName?: string;
  splitType: SplitType;
  splits?: ExpenseSplit[];
}

export interface CreateExpenseRequest {
  description: string;
  amount: number;
  paidById: number;
  groupId: number;
  splitType: SplitType;
  splits: ExpenseSplit[];
}