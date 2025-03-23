import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserConectionComponent } from './user-conection.component';

describe('UserConectionComponent', () => {
  let component: UserConectionComponent;
  let fixture: ComponentFixture<UserConectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserConectionComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UserConectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
