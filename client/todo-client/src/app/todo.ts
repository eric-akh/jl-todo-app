export enum TodoPriority {
  Low = 1,
  Medium = 2,
  High = 3,
}

export interface TodoItem {
  id: string;
  title: string;
  isCompleted: boolean;
  priority: TodoPriority; // 1..3
  createdAt: string;      // ISO string
  dueAt?: string | null;  // ISO string or null
}

export interface CreateTodoRequest {
  title: string;
  priority: TodoPriority;
  dueAt?: string | null;
}
