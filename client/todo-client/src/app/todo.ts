// Domain types used by the UI.
// Keeping priority as 1|2|3 matches the API payload cleanly.

export type TodoPriority = 1 | 2 | 3;

export interface TodoItem {
  id: string;
  title: string;
  isCompleted: boolean;
  priority: TodoPriority;
  createdAt: string;
  dueAt: string;
}

export const PRIORITIES: Array<{ value: TodoPriority; label: string }> = [
  { value: 1, label: 'Low' },
  { value: 2, label: 'Medium' },
  { value: 3, label: 'High' }
];
