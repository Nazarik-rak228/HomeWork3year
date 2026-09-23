package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.ReviewDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ReviewRepository extends JpaRepository<ReviewDto, Integer> {
    List<ReviewDto> findByRating(int rating);
}
/*

@Repository

public class ReviewRepository {
    private List<ReviewModel> reviews = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<ReviewModel> findAll() {
        return new ArrayList<>(reviews);
    }

    public ReviewModel addReview(ReviewModel review) {
        review.setId(idCounter.getAndIncrement());
        reviews.add(review);
        return review;
    }

    public ReviewModel updateReview(ReviewModel review) {
        for (int i = 0; i < reviews.size(); i++) {
            if (reviews.get(i).getId() == review.getId()) {
                reviews.set(i, review);
                return review;
            }
        }
        return null;
    }

    public void deleteReview(int id) {
        reviews.removeIf(review -> review.getId() == id);
    }

    public ReviewModel findById(int id) {
        return reviews.stream()
                .filter(review -> review.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<ReviewModel> findByRating(int rating) {
        return reviews.stream()
                .filter(r -> r.getRating() == rating)
                .toList();
    }
}
*/
