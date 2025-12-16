import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TodoItem, TodoPriority } from './todo';

export interface CreateTodoRequest {
  title: string;
  priority: TodoPriority;
  dueAt: string; // required now
}

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly baseUrl = 'http://localhost:5028/api/todos';

  constructor(private readonly http: HttpClient) { }

  getAll(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.baseUrl);
  }

  add(request: CreateTodoRequest): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.baseUrl, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggle(id: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/toggle`, {});
  }
}
