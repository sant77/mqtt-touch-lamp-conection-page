import { Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { SignInComponent } from './components/sign-in/sign-in.component';
import { HomeComponent } from './components/home/home.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuard } from './auth.guard';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

export const routes: Routes = [
    { path: '', component: HomeComponent }, // Página principal
    { path: 'sign-up', component: SignUpComponent }, // Página de registro
    { path: 'sign-in', component: SignInComponent }, // Página de inicio de sesión
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: '**', redirectTo: '' } // Redirige cualquier ruta no existente a la página principal
  ];
