# SmartWatcher

A simple windows service designed to watch a specific directories and taking specific actions to : Create - Change - Rename - Delete files events into those directories. The advantage of this tool:
Supporting plugins, just code about you business (what you want to do with file) compile your code into dll and let Smart Watcher take care about files watching.
Check if file is used by another service and wait before processing it (very useful specially when you are working with large files when coping or downloading them).
Support multiple file extensions and filters.
Using Regular Expressions for files filtering.
Watch many directories at the same time.
Creating a queue if many files created at the same time.
Archive the files into password protected zip files in case of success or failure.
SmartTimer: a feature which implements a timer that raises an event at user-defined intervals. Now SmartWatcher support the Tick event which regularly invokes code. Every several seconds or minutes, Just handle this event in your plugin and SmartWatcher will invoke your code.



[![Gitter](https://badges.gitter.im/TimCoogan/SmartWatcher.svg)](https://gitter.im/TimCoogan/SmartWatcher?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
