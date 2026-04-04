// ─────────────────────────────────────────────────────────────────────────
// shell.component.ts — Application Shell
//
// The Shell is the persistent outer wrapper for all authenticated pages.
// It contains:
//   - Top navigation bar (header)
//   - Side navigation (sidebar)
//   - Main content area (router-outlet)
//
// Layout:
// ┌──────────────────────────────────────────────┐
// │  HEADER (fixed, full width)                  │
// ├─────────────┬────────────────────────────────┤
// │  SIDEBAR    │  MAIN CONTENT (router-outlet)  │
// │  (fixed)    │                                │
// │             │                                │
// └─────────────┴────────────────────────────────┘
// ─────────────────────────────────────────────────────────────────────────

import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { Sidebar } from '../sidebar/sidebar';
import { Auth } from '../../core/auth/auth';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    Sidebar,
  ],
  templateUrl: './shell.html',
  styleUrls: ['./shell.scss'],
})
export class Shell implements OnInit {
  private readonly authService = inject(Auth);

  // Reactive state using Angular Signals
  readonly isSidebarOpen = signal(true);
  readonly currentUser = this.authService.currentUser;
  readonly userInitials = computed(() => {
    const user = this.currentUser();
    if (!user) return '?';
    return user.name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  });

  ngOnInit(): void {
    // Auto-close sidebar on small screens
    if (window.innerWidth < 768) {
      this.isSidebarOpen.set(false);
    }
  }

  toggleSidebar(): void {
    this.isSidebarOpen.update((open) => !open);
  }

  async logout(): Promise<void> {
    await this.authService.logout();
  }
}
