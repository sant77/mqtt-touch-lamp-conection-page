import { Component } from '@angular/core';
import { NavbarComponent } from './navbar/navbar.component';
import { ContentComponent } from './content/content.component';
import { FooComponent } from './foo/foo.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NavbarComponent, ContentComponent, FooComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
