import { Component, ChangeDetectionStrategy, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { TodoService } from './todo.service';
import { TodoItem, TodoPriority } from './todo';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from './confirm-dialog.component';

@Component({
  selector: 'app-root',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DatePipe,

    MatToolbarModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCheckboxModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatTooltipModule,

    // Needed for MatDialog service + dialog directives
    MatDialogModule,

    ConfirmDialogComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private readonly service = inject(TodoService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly dialog = inject(MatDialog);

  titleText = 'Todo List';
  loading = false;
  errorMessage = '';

  todos: TodoItem[] = [];
  displayedColumns: string[] = ['done', 'title', 'priority', 'due', 'actions'];

  priorities = [
    { value: 1 as TodoPriority, label: 'Low' },
    { value: 2 as TodoPriority, label: 'Medium' },
    { value: 3 as TodoPriority, label: 'High' }
  ];

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    priority: [2 as TodoPriority, [Validators.required]],
    // Due date must be provided
    dueAt: [null as Date | null, [Validators.required]]
  });

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.errorMessage = '';
    this.loading = true;

    this.service.getAll()
      .pipe(finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      }))
      .subscribe({
        next: (items) => {
          this.todos = items ?? [];
          this.cdr.markForCheck();
        },
        error: () => {
          this.errorMessage = 'Could not load todos. Please try again.';
          this.cdr.markForCheck();
        }
      });
  }

  add(): void {
    if (this.form.invalid || this.loading) return;

    const title = (this.form.value.title ?? '').trim();
    if (!title) return;

    const priority = (this.form.value.priority ?? 2) as TodoPriority;
    const dueAtDate = this.form.value.dueAt ?? null;

    // Form requires due date, but keep a defensive check.
    if (!dueAtDate) return;

    this.errorMessage = '';
    this.loading = true;

    this.service.add({
      title,
      priority,
      dueAt: dueAtDate.toISOString()
    })
      .pipe(finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      }))
      .subscribe({
        next: () => {
          this.form.reset({ title: '', priority: 2, dueAt: null });
          this.refresh();
        },
        error: () => {
          this.errorMessage = 'Could not add todo. Please try again.';
          this.cdr.markForCheck();
        }
      });
  }

  toggle(todo: TodoItem): void {
    if (this.loading) return;

    this.loading = true;
    this.service.toggle(todo.id)
      .pipe(finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      }))
      .subscribe({
        next: () => this.refresh(),
        error: () => {
          this.errorMessage = 'Could not update todo.';
          this.cdr.markForCheck();
        }
      });
  }

  remove(todo: TodoItem): void {
    // Ask user confirmation before deletion.
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: todo.title },
      // Optional UX tweaks:
      disableClose: true,
      autoFocus: false
    });

    ref.afterClosed().subscribe((confirmed: boolean) => {
      if (!confirmed || this.loading) return;

      this.loading = true;
      this.errorMessage = '';

      this.service.delete(todo.id)
        .pipe(finalize(() => {
          this.loading = false;
          this.cdr.markForCheck();
        }))
        .subscribe({
          next: () => this.refresh(),
          error: () => {
            this.errorMessage = 'Failed to delete todo.';
            this.cdr.markForCheck();
          }
        });
    });
  }

  priorityChipClass(priority: number): string {
    if (priority === 1) return 'chip-low';
    if (priority === 2) return 'chip-medium';
    return 'chip-high';
  }

  priorityLabel(p: number): string {
    switch (p) {
      case 1: return 'Low';
      case 2: return 'Medium';
      case 3: return 'High';
      default: return 'Unknown';
    }
  }

  createdAtTooltip(createdAt: string | Date): string {
    const d = typeof createdAt === 'string' ? new Date(createdAt) : createdAt;
    return `Created: ${d.toLocaleString()}`;
  }

  dueText(dueAt?: string | null): string {
    if (!dueAt) return '-';
    const d = new Date(dueAt);
    return isNaN(d.getTime()) ? '-' : d.toLocaleDateString();
  }
}
