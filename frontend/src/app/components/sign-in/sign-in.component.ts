import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { LoginUser } from './login-service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [CommonModule, 
            MatInputModule, 
            MatButtonModule, 
            ReactiveFormsModule
          ],
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent {
  registerForm: FormGroup;
    errorMessage: string | null = null; // Para manejar mensajes de error
  
    constructor(
      private fb: FormBuilder,
      private myService: LoginUser,
      private router: Router, // Inyectar el Router
      private toastr: ToastrService
    
    ) {
      this.registerForm = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required]
      });
    }
  
    onSubmit(): void {
      if (this.registerForm.valid) {
        const formData = this.registerForm.value;
        this.myService.login(formData).subscribe({
          next: (response) => {
            localStorage.setItem('token', response.token);
            console.log('Registro exitoso', response);
            this.router.navigate(['/dashboard']); // Redirigir al dashboard
          },
          error: (err) => {
            this.toastr.error('Usuario o contraseña incorrecta', '¡Error!');
            console.error('Error en el registro', err);
          }
        });
      } else {
       
        this.errorMessage = 'Por favor completa todos los campos correctamente.'; // Validación en el cliente
      }
    }
}
