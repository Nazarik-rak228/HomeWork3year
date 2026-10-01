package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.TaskDto;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.dto.CategoryDto;
import org.example.creatfirtsapi.repository.TaskRepository;
import org.example.creatfirtsapi.repository.UserRepository;
import org.example.creatfirtsapi.repository.CategoryRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/task")
@Tag(name = "Tasks", description = "CRUD для задач")
public class TaskController {

    private final TaskRepository taskRepository;
    private final UserRepository userRepository;
    private final CategoryRepository categoryRepository;

    public TaskController(TaskRepository taskRepository,
                          UserRepository userRepository,
                          CategoryRepository categoryRepository) {
        this.taskRepository = taskRepository;
        this.userRepository = userRepository;
        this.categoryRepository = categoryRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все задачи", description = "Возвращает список всех задач")
    @GetMapping
    public List<TaskDto> getAll() {
        return taskRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить задачу по id", description = "Возвращает задачу по её id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return taskRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Task not found"));
    }

    // CREATE
    @Operation(summary = "Создать задачу", description = "Создаёт задачу с пользователем и категорией")
    @PostMapping
    public ResponseEntity addTask(@RequestParam String title,
                                  @RequestParam String description,
                                  @RequestParam String deadline,
                                  @RequestParam double price,
                                  @RequestParam int userId,
                                  @RequestParam int categoryId) {

        try {
            LocalDate date = LocalDate.parse(deadline);

            UserDto user = userRepository.findById(userId).orElse(null);
            CategoryDto category = categoryRepository.findById(categoryId).orElse(null);

            if (user == null || category == null) {
                return ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("User or Category not found");
            }

            TaskDto task = new TaskDto(0, title, description, date, price, user, category);
            task.setStatus("Открыто");

            taskRepository.save(task);
            return ResponseEntity.status(HttpStatus.CREATED).body(task);

        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body("Invalid data: " + e.getMessage());
        }
    }

    // UPDATE
    @Operation(summary = "Обновить задачу", description = "Обновляет задачу по id")
    @PutMapping
    public ResponseEntity updateTask(@RequestParam int id,
                                     @RequestParam String title,
                                     @RequestParam String description,
                                     @RequestParam String deadline,
                                     @RequestParam double price,
                                     @RequestParam int userId,
                                     @RequestParam int categoryId,
                                     @RequestParam(required = false) String status) {

        if (!taskRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Task not found");
        }

        try {
            LocalDate date = LocalDate.parse(deadline);

            UserDto user = userRepository.findById(userId).orElse(null);
            CategoryDto category = categoryRepository.findById(categoryId).orElse(null);

            if (user == null || category == null) {
                return ResponseEntity.status(HttpStatus.NOT_FOUND)
                        .body("User or Category not found");
            }

            TaskDto task = new TaskDto(
                    id,
                    title,
                    description,
                    date,
                    price,
                    user,
                    category
            );
            if (status == null || status.isBlank()) {
                task.setStatus(taskRepository.findById(id).map(TaskDto::getStatus).orElse(null));
            } else {
                task.setStatus(status);
            }

            taskRepository.save(task);
            return ResponseEntity.ok(task);

        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body("Invalid date format");
        }
    }

    // DELETE
    @Operation(summary = "Удалить задачу", description = "Удаляет задачу по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteTask(@PathVariable int id) {

        if (!taskRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Task not found");
        }

        taskRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}