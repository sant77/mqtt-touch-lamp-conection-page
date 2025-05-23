import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './auth.interceptor'; // Importa el interceptor como función
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';


export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(), // required animations providers
    provideToastr(),
    provideRouter(routes), // Configura las rutas
    provideAnimationsAsync(), // Habilita animaciones asíncronas
    provideHttpClient(
      withFetch(), // Habilita el uso de fetch en lugar de XHR
      withInterceptors([authInterceptor]) // Registra el interceptor como función
    )
  ]
};

