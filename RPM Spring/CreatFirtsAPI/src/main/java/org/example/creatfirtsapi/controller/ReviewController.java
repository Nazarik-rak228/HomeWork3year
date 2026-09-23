package org.example.creatfirtsapi.controller;

import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.example.creatfirtsapi.dto.ReviewDto;
import org.example.creatfirtsapi.dto.UserDto;
import org.example.creatfirtsapi.repository.ReviewRepository;
import org.example.creatfirtsapi.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/review")
@Tag(name = "Reviews", description = "CRUD для отзывов")
public class ReviewController {

    private final ReviewRepository reviewRepository;
    private final UserRepository userRepository;

    public ReviewController(ReviewRepository reviewRepository,
                            UserRepository userRepository) {
        this.reviewRepository = reviewRepository;
        this.userRepository = userRepository;
    }

    // GET ALL
    @Operation(summary = "Получить все отзывы", description = "Возвращает список всех отзывов")
    @GetMapping
    public List<ReviewDto> getAll() {
        return reviewRepository.findAll();
    }

    // GET BY ID
    @Operation(summary = "Получить отзыв по id", description = "Возвращает отзыв по его id")
    @GetMapping("/{id}")
    public ResponseEntity getById(@PathVariable int id) {
        return reviewRepository.findById(id)
                .<ResponseEntity>map(ResponseEntity::ok)
                .orElseGet(() -> ResponseEntity.status(HttpStatus.NOT_FOUND).body("Review not found"));
    }

    // CREATE
    @Operation(summary = "Создать отзыв", description = "Создаёт отзыв между пользователями с рейтингом")
    @PostMapping
    public ResponseEntity addReview(@RequestParam int outUserId,
                                    @RequestParam int inUserId,
                                    @RequestParam int rating,
                                    @RequestParam String comment) {

        UserDto outUser = userRepository.findById(outUserId).orElse(null);
        UserDto inUser = userRepository.findById(inUserId).orElse(null);

        if (outUser == null || inUser == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User not found");
        }

        ReviewDto review = new ReviewDto(0, outUser, inUser, rating, comment);

        reviewRepository.save(review);
        return ResponseEntity.status(HttpStatus.CREATED).body(review);
    }

    // UPDATE
    @Operation(summary = "Обновить отзыв", description = "Обновляет отзыв по id")
    @PutMapping
    public ResponseEntity updateReview(@RequestParam int id,
                                       @RequestParam int outUserId,
                                       @RequestParam int inUserId,
                                       @RequestParam int rating,
                                       @RequestParam String comment) {

        if (!reviewRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Review not found");
        }

        UserDto outUser = userRepository.findById(outUserId).orElse(null);
        UserDto inUser = userRepository.findById(inUserId).orElse(null);

        if (outUser == null || inUser == null) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("User not found");
        }

        ReviewDto review = new ReviewDto(
                id,
                outUser,
                inUser,
                rating,
                comment
        );

        reviewRepository.save(review);
        return ResponseEntity.ok(review);
    }

    // DELETE
    @Operation(summary = "Удалить отзыв", description = "Удаляет отзыв по id")
    @DeleteMapping("/{id}")
    public ResponseEntity deleteReview(@PathVariable int id) {

        if (!reviewRepository.existsById(id)) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND)
                    .body("Review not found");
        }

        reviewRepository.deleteById(id);
        return ResponseEntity.ok("Deleted");
    }
}