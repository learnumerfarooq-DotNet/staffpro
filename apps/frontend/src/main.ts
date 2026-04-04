// ─────────────────────────────────────────────────────────────────────────
// main.ts — Application Bootstrap
//
// This is the very first file executed when the app loads in the browser.
// It bootstraps the root Angular component using the app configuration.
// ─────────────────────────────────────────────────────────────────────────

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app';

bootstrapApplication(AppComponent, appConfig).catch((err) => console.error(err));
