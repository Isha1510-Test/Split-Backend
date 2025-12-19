import { User } from './user.model';

export interface Group {
  id?: number;
  name: string;
  description?: string;
  createdById?: number;
  createdByName?: string;
  members?: User[];
  memberIds?: number[];
}

export interface CreateGroupRequest {
  name: string;
  description?: string;
  createdById: number;
  memberIds: number[];
}