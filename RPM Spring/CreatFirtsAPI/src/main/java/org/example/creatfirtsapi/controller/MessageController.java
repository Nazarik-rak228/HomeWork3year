package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.MessageDto;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.repository.MessageRepository;
import org.example.creatfirtsapi.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

@RestController
@RequestMapping("/api/message")
@Tag(name = "Messages", description = "CRUD для сообщений")
public class MessageController {

    private final MessageRepository messageRepository;
    private final UserRepository userRepository;

    public MessageController(MessageRepository messageRepository,
                             UserRepository userRepository) {
        this.messageRepository = messageRepository;
        this.userRepository = userRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все сообщения", description = "Возвращает список всех сообщений")
    @GetMapping
    public List<MessageDto> getAll() {
        return messageRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить сообщение по id", description = "Возвращает сообщение по его id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return messageRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Message not found"));
    }

    // CREATE
    @Operation(summary = "Создать сообщение", description = "Создаёт сообщение между пользователями")
    @PostMapping
    public ResponseEntity addMessage(@RequestParam int senderId,
                                     @RequestParam int receiverId,
                                     @RequestParam String text) {

        UserDto sender = userRepository.findById(senderId).orElse(null);
        UserDto receiver = userRepository.findById(receiverId).orElse(null);

        if (sender == null || receiver == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Sender or receiver not found");
        }

        MessageDto message = new MessageDto(0, sender, receiver, text, LocalDateTime.now());

        messageRepository.save(message);
        return ResponseEntity.status(HttpStatus.CREATED).body(message);
    }

    // UPDATE
    @Operation(summary = "Обновить сообщение", description = "Обновляет текст сообщения")
    @PutMapping
    public ResponseEntity updateMessage(@RequestParam int id,
                                        @RequestParam int senderId,
                                        @RequestParam int receiverId,
                                        @RequestParam String text) {

        if (!messageRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Message not found");
        }

        UserDto sender = userRepository.findById(senderId).orElse(null);
        UserDto receiver = userRepository.findById(receiverId).orElse(null);

        if (sender == null || receiver == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Sender or receiver not found");
        }

        MessageDto message = new MessageDto(
                id,
                sender,
                receiver,
                text,
                LocalDateTime.now()
        );

        messageRepository.save(message);
        return ResponseEntity.ok(message);
    }

    // DELETE
    @Operation(summary = "Удалить сообщение", description = "Удаляет сообщение по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteMessage(@PathVariable int id) {

        if (!messageRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Message not found");
        }

        messageRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}