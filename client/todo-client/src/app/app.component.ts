import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TodoService } from './todo.service';
import { Todo } from './todo';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private todoService = inject(TodoService);

  todos: Todo[] = [];
  newTitle = '';
  loading = false;
  error = '';

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.loading = true;
    this.error = '';
    this.todoService.getTodos().subscribe({
      next: data => {
        this.todos = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load todos';
        this.loading = false;
      }
    });
  }

  addTodo(): void {
    const title = this.newTitle.trim();
    if (!title) {
      return;
    }

    this.todoService.addTodo(title).subscribe({
      next: todo => {
        this.todos.push(todo);
        this.newTitle = '';
      },
      error: () => {
        this.error = 'Failed to add todo';
      }
    });
  }

  deleteTodo(todo: Todo): void {
    this.todoService.deleteTodo(todo.id).subscribe({
      next: () => {
        this.todos = this.todos.filter(t => t.id !== todo.id);
      },
      error: () => {
        this.error = 'Failed to delete todo';
      }
    });
  }
}
