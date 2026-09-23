package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.ResponseDto;
import org.example.creatfirtsapi.dto.TaskDto;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.repository.ResponseRepository;
import org.example.creatfirtsapi.repository.TaskRepository;
import org.example.creatfirtsapi.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/response")
@Tag(name = "Responses", description = "CRUD для откликов")
public class ResponseController {

    private final ResponseRepository responseRepository;
    private final UserRepository userRepository;
    private final TaskRepository taskRepository;

    public ResponseController(ResponseRepository responseRepository,
                              UserRepository userRepository,
                              TaskRepository taskRepository) {
        this.responseRepository = responseRepository;
        this.userRepository = userRepository;
        this.taskRepository = taskRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все отклики", description = "Возвращает список всех откликов")
    @GetMapping
    public List<ResponseDto> getAll() {
        return responseRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить отклик по id", description = "Возвращает отклик по его id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return responseRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Response not found"));
    }

    // CREATE
    @Operation(summary = "Создать отклик", description = "Создаёт отклик пользователя на задачу")
    @PostMapping
    public ResponseEntity addResponse(@RequestParam int userId,
                                      @RequestParam int taskId,
                                      @RequestParam String message,
                                      @RequestParam String status) {

        UserDto user = userRepository.findById(userId).orElse(null);
        TaskDto task = taskRepository.findById(taskId).orElse(null);

        if (user == null || task == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User or Task not found");
        }

        ResponseDto response = new ResponseDto(0, user, task, message, status);

        responseRepository.save(response);
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }

    // UPDATE
    @Operation(summary = "Обновить отклик", description = "Обновляет отклик по id")
    @PutMapping
    public ResponseEntity updateResponse(@RequestParam int id,
                                         @RequestParam int userId,
                                         @RequestParam int taskId,
                                         @RequestParam String message,
                                         @RequestParam String status) {

        if (!responseRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Response not found");
        }

        UserDto user = userRepository.findById(userId).orElse(null);
        TaskDto task = taskRepository.findById(taskId).orElse(null);

        if (user == null || task == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User or Task not found");
        }

        ResponseDto response = new ResponseDto(
                id,
                user,
                task,
                message,
                status
        );

        responseRepository.save(response);
        return ResponseEntity.ok(response);
    }

    // DELETE
    @Operation(summary = "Удалить отклик", description = "Удаляет отклик по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteResponse(@PathVariable int id) {

        if (!responseRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Response not found");
        }

        responseRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}