import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TodoService } from './todo.service';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TodoService]
    });

    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('getAll() should call API and return todos', () => {
    const mockTodos = [
      { id: '1', title: 'A', priority: 2, dueAt: new Date().toISOString(), createdAt: new Date().toISOString(), isCompleted: false }
    ];

    service.getAll().subscribe(todos => {
      expect(todos.length).toBe(1);
      expect(todos[0].title).toBe('A');
    });

    const req = httpMock.expectOne(r => r.method === 'GET');
    req.flush(mockTodos);
  });
});
