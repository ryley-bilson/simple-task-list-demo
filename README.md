### Todo
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

### Changelog
v0.1.0
- Initial release
- Separated quicksort from `Queue.cs`
- Changed quicksort methods to generic
  - Now requires object type to implement `IComparable`
