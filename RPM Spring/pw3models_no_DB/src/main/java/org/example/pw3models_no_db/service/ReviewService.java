package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.ReviewModel;

import java.util.List;

public interface ReviewService {
    List<ReviewModel> findAll();
    ReviewModel addReview(ReviewModel review);
    ReviewModel updateReview(ReviewModel review);
    void deleteReview(int id);
    List<ReviewModel> findByRating(int rating);
    ReviewModel findById(int id);
}