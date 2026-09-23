package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.example.creatfirtsapi.dto.TaskDto;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.dto.UserTaskDto;
import org.example.creatfirtsapi.repository.TaskRepository;
import org.example.creatfirtsapi.repository.UserRepository;
import org.example.creatfirtsapi.repository.UserTaskRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/usertask")
@Tag(name = "User Tasks", description = "Связь пользователей и задач")
public class UserTaskController {

    private final UserTaskRepository userTaskRepository;
    private final UserRepository userRepository;
    private final TaskRepository taskRepository;

    public UserTaskController(UserTaskRepository userTaskRepository,
                              UserRepository userRepository,
                              TaskRepository taskRepository) {
        this.userTaskRepository = userTaskRepository;
        this.userRepository = userRepository;
        this.taskRepository = taskRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все связи user-task", description = "Возвращает все назначения задач пользователям")
    @GetMapping
    public List<UserTaskDto> getAll() {
        return userTaskRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить связь по id", description = "Возвращает назначение задачи по id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return userTaskRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("UserTask not found"));
    }

    // CREATE
    @Operation(summary = "Назначить задачу пользователю", description = "Создаёт связь user-task со статусом")
    @PostMapping
    public ResponseEntity addUserTask(@RequestParam int userId,
                                      @RequestParam int taskId,
                                      @RequestParam String status) {

        UserDto user = userRepository.findById(userId).orElse(null);
        TaskDto task = taskRepository.findById(taskId).orElse(null);

        if (user == null || task == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User or Task not found");
        }

        UserTaskDto userTask = new UserTaskDto(0, user, task, status);

        userTaskRepository.save(userTask);
        return ResponseEntity.status(HttpStatus.CREATED).body(userTask);
    }

    // UPDATE
    @Operation(summary = "Обновить назначение", description = "Обновляет статус или связь user-task")
    @PutMapping
    public ResponseEntity updateUserTask(@RequestParam int id,
                                         @RequestParam int userId,
                                         @RequestParam int taskId,
                                         @RequestParam String status) {

        if (!userTaskRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("UserTask not found");
        }

        UserDto user = userRepository.findById(userId).orElse(null);
        TaskDto task = taskRepository.findById(taskId).orElse(null);

        if (user == null || task == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User or Task not found");
        }

        UserTaskDto userTask = new UserTaskDto(
                id,
                user,
                task,
                status
        );

        userTaskRepository.save(userTask);
        return ResponseEntity.ok(userTask);
    }

    // DELETE
    @Operation(summary = "Удалить назначение", description = "Удаляет связь user-task по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteUserTask(@PathVariable int id) {

        if (!userTaskRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("UserTask not found");
        }

        userTaskRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}