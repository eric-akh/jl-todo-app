import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo } from './todo';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:5028/api/todos'; // Set API base URL here

  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.baseUrl);
  }

  addTodo(title: string): Observable<Todo> {
    const body = { id: 0, title, isDone: false };
    return this.http.post<Todo>(this.baseUrl, body);
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
