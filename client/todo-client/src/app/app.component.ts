import { Component, OnInit, ViewChild, inject, NgZone, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroupDirective, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { of, catchError, finalize, map, switchMap, timeout } from 'rxjs';

import { TodoItem, TodoPriority, PRIORITIES } from './todo';
import { TodoService } from './todo.service';
import { ConfirmDialogComponent } from './confirm-dialog.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatToolbarModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTableModule,
    MatCheckboxModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDialogModule,

    ConfirmDialogComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Todo';
  todos: TodoItem[] = [];
  displayedColumns = ['done', 'title', 'priority', 'due', 'created', 'actions'];

  priorities = PRIORITIES;

  loading = false;
  errorMessage = '';

  private readonly fb = inject(FormBuilder);
  private readonly service = inject(TodoService);
  private readonly dialog = inject(MatDialog);
  private readonly zone = inject(NgZone);
  private readonly cdr = inject(ChangeDetectorRef);

  // Resetting this clears FormGroupDirective.submitted, which prevents red errors after refresh.
  @ViewChild(FormGroupDirective) private formDir?: FormGroupDirective;

  form = this.fb.group({
    title: ['', [Validators.required]],
    priority: [2 as TodoPriority, [Validators.required]],
    dueAt: [null as Date | null, [Validators.required]]
  });

  ngOnInit(): void {
    this.refresh();
  }

  private sortTodos(items: TodoItem[]): TodoItem[] {
    return [...items].sort((a, b) => {
      const da = new Date(a.dueAt).getTime();
      const db = new Date(b.dueAt).getTime();
      if (da !== db) return da - db;
      return a.title.localeCompare(b.title);
    });
  }

  private clearFormErrorUiKeepValues(): void {
    const v = this.form.getRawValue();
    this.formDir?.resetForm(v); // clears submitted + touched/dirty flags in the directive layer
    this.form.reset(v, { emitEvent: false });
  }

  refresh(): void {
    this.loading = true;
    this.errorMessage = '';

    this.service
      .getAll()
      .pipe(
        timeout(10000),
        map((items) => this.sortTodos(items)),
        catchError(() => {
          this.zone.run(() => {
            this.errorMessage = 'Failed to load todos. Please try again.';
          });
          return of([] as TodoItem[]);
        }),
        finalize(() => {
          this.zone.run(() => {
            this.loading = false;
            this.clearFormErrorUiKeepValues();
            this.cdr.markForCheck();
          });
        })
      )
      .subscribe((items) => {
        this.zone.run(() => {
          this.todos = items;
          this.cdr.markForCheck();
        });
      });
  }

  add(): void {
    this.errorMessage = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const title = (this.form.value.title ?? '').trim();
    const priority = this.form.value.priority as TodoPriority;
    const dueDate = this.form.value.dueAt as Date;

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const dueOnly = new Date(dueDate);
    dueOnly.setHours(0, 0, 0, 0);

    if (dueOnly.getTime() <= today.getTime()) {
      this.errorMessage = 'Due date must be in the future.';
      return;
    }

    const dueAtIso = new Date(
      Date.UTC(dueOnly.getFullYear(), dueOnly.getMonth(), dueOnly.getDate(), 0, 0, 0)
    ).toISOString();

    this.loading = true;

    this.service
      .add({ title, priority, dueAt: dueAtIso })
      .pipe(
        timeout(10000),
        switchMap(() => this.service.getAll()),
        timeout(10000),
        map((items) => this.sortTodos(items)),
        catchError(() => {
          this.zone.run(() => {
            this.errorMessage = 'Failed to add todo.';
          });
          return of(null);
        }),
        finalize(() => {
          this.zone.run(() => {
            this.loading = false;
            this.cdr.markForCheck();
          });
        })
      )
      .subscribe((items) => {
        if (!items) return;

        this.zone.run(() => {
          this.todos = items;

          const initial = { title: '', priority: 2 as TodoPriority, dueAt: null as Date | null };
          this.formDir?.resetForm(initial);
          this.form.reset(initial, { emitEvent: false });

          this.cdr.markForCheck();
        });
      });
  }

  toggle(todo: TodoItem): void {
    this.errorMessage = '';

    this.service
      .toggle(todo.id)
      .pipe(timeout(10000))
      .subscribe({
        next: () => this.refresh(),
        error: () => {
          this.zone.run(() => {
            this.errorMessage = 'Failed to update todo.';
            this.cdr.markForCheck();
          });
        }
      });
  }

  confirmDelete(todo: TodoItem): void {
    this.errorMessage = '';

    this.dialog
      .open(ConfirmDialogComponent, {
        data: { message: `Delete "${todo.title}"?` }
      })
      .afterClosed()
      .subscribe((confirmed: boolean) => {
        if (confirmed !== true) return;

        this.service
          .delete(todo.id)
          .pipe(timeout(10000))
          .subscribe({
            next: () => this.refresh(),
            error: () => {
              this.zone.run(() => {
                this.errorMessage = 'Failed to delete todo.';
                this.cdr.markForCheck();
              });
            }
          });
      });
  }

  priorityLabel(p: number): string {
    if (p === 1) return 'Low';
    if (p === 2) return 'Medium';
    return 'High';
  }

  priorityChipClass(p: number): string {
    if (p === 1) return 'chip chip-low';
    if (p === 2) return 'chip chip-med';
    return 'chip chip-high';
  }
}
