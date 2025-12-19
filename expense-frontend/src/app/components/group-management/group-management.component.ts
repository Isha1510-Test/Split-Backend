import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import { Group, CreateGroupRequest } from '../../models/group.model';
import { GroupService } from '../../services/group.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-group-management',
  templateUrl: './group-management.component.html',
  styleUrls: ['./group-management.component.scss']
})
export class GroupManagementComponent implements OnInit {
  @Input() currentUser: User | null = null;
  @Output() groupSelected = new EventEmitter<Group>();

  groups: Group[] = [];
  allUsers: User[] = [];
  showCreateForm = false;
  loading = false;
  
  newGroup: CreateGroupRequest = {
    name: '',
    description: '',
    createdById: 0,
    memberIds: []
  };

  constructor(
    private groupService: GroupService,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.loadGroups();
    this.loadUsers();
  }

  ngOnChanges() {
    if (this.currentUser) {
      this.newGroup.createdById = this.currentUser.id!;
    }
  }

  loadGroups() {
    this.loading = true;
    this.groupService.getAllGroups().subscribe({
      next: (groups) => {
        this.groups = groups;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading groups:', error);
        this.loading = false;
      }
    });
  }

  loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.allUsers = users;
      },
      error: (error) => {
        console.error('Error loading users:', error);
      }
    });
  }

  createGroup() {
    if (!this.newGroup.name || !this.currentUser) {
      alert('Please fill in all required fields');
      return;
    }

    this.newGroup.createdById = this.currentUser.id!;
    
    this.groupService.createGroup(this.newGroup).subscribe({
      next: (group) => {
        this.groups.push(group);
        this.resetForm();
        alert('Group created successfully!');
      },
      error: (error) => {
        console.error('Error creating group:', error);
        alert('Error creating group. Please try again.');
      }
    });
  }

  selectGroup(group: Group) {
    this.groupSelected.emit(group);
  }

  deleteGroup(group: Group) {
    if (confirm(`Are you sure you want to delete ${group.name}?`)) {
      this.groupService.deleteGroup(group.id!).subscribe({
        next: () => {
          this.groups = this.groups.filter(g => g.id !== group.id);
          alert('Group deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting group:', error);
          alert('Error deleting group. Please try again.');
        }
      });
    }
  }

  toggleMember(userId: number) {
    const index = this.newGroup.memberIds.indexOf(userId);
    if (index > -1) {
      this.newGroup.memberIds.splice(index, 1);
    } else {
      this.newGroup.memberIds.push(userId);
    }
  }

  isMemberSelected(userId: number): boolean {
    return this.newGroup.memberIds.includes(userId);
  }

  resetForm() {
    this.newGroup = {
      name: '',
      description: '',
      createdById: this.currentUser?.id || 0,
      memberIds: []
    };
    this.showCreateForm = false;
  }
}