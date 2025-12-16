import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { AppComponent } from './app.component';
import { TodoService } from './todo.service';
import { MatDialog } from '@angular/material/dialog';

describe('AppComponent', () => {
  const todoServiceMock = {
    getAll: vi.fn(() => of([])),
    add: vi.fn(() => of({})),
    toggle: vi.fn(() => of({})),
    delete: vi.fn(() => of({}))
  };

  const matDialogMock = {
    open: vi.fn(() => ({
      afterClosed: () => of(false)
    }))
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        { provide: TodoService, useValue: todoServiceMock },
        { provide: MatDialog, useValue: matDialogMock }
      ]
    }).compileComponents();

    vi.clearAllMocks();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(AppComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should call refresh on init and stop loading', () => {
    const fixture = TestBed.createComponent(AppComponent);

    // triggers ngOnInit -> refresh()
    fixture.detectChanges();

    const comp = fixture.componentInstance;

    expect(todoServiceMock.getAll).toHaveBeenCalled();
    expect(comp.loading).toBe(false); // Vitest matcher
  });
});
