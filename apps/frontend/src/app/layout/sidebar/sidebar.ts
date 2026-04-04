import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

export interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
  children?: NavItem[];
  month?: number;
  disabled?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatListModule,
    MatIconModule,
    MatDividerModule,
  ],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.scss'],
})
export class Sidebar {
  readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Company Setup', icon: 'business', route: '/company/setup' },
    { label: 'Clients', icon: 'people', route: '/clients', disabled: true },
    { label: 'Employees', icon: 'badge', route: '/employees', disabled: true },
    { label: 'Jobs', icon: 'work', route: '/jobs', disabled: true },
    { label: 'Candidates', icon: 'person_search', route: '/candidates', disabled: true },
    { label: 'Careers', icon: 'trending_up', route: '/careers', disabled: true },
  ];
}
