import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router); // Inyecta el Router

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        // Token vencido o no autorizado
        localStorage.removeItem('token'); // Eliminar el token vencido
        router.navigate(['/sign-in']); // Redirigir al inicio de sesión
      }
      return throwError(() => error); // Reenviar el error
    })
  );
};