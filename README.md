## Todo
- [x] TODO list
- [x] Reintegrate as new project
- [x] Rework quicksort method to generic
  - [x] Implement `IComparable` in QueueNode
- [ ] Change task to utilise only date (ie. not time)
- [ ] Integrate priority (enum) into functionality
- [ ] Plan UI changes
  - [ ] Add toolbar
  - [ ] Add status strip
  - [ ] Add Task.Priority to display table
  - [ ] Add Task.Priority to editing fields
  - [ ] Add tooltips
- [ ] `isUnsaved` (ie. "Are you sure you want to [quit/load/etc.]?")
- [ ] Unit tests (FULL/EXTREME coverage)

## Changelog
### [major release 3, less soon yet still]
- UI cleanup  
- new UI features

### [major release 2, less soon]
- Task `dueDateTime` reworked into `dueDate`
  - UI modified to accommodate
- Relevance granted to `(TaskPriority) priority`

### [major release 1, soon]
- Initial release
- Separated quicksort from `Queue.cs`
- Changed quicksort methods to generic
  - Now functions for `(List<T>) array where T : System.IComparable`
