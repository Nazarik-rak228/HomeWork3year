package org.example.creatfirtsapi.dto;


import jakarta.persistence.*;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;

@Entity
@Table(name = "reviews")
public class ReviewDto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    // Кто оставил отзыв
    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "out_user_id", nullable = false)
    private UserDto outUser;

    // Кому оставили отзыв
    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "in_user_id", nullable = false)
    private UserDto inUser;

    @Min(1)
    @Max(5)
    private int rating;

    private String comment;

    public ReviewDto() {
    }

    public ReviewDto(int id, UserDto outUser, UserDto inUser, int rating, String comment) {
        this.id = id;
        this.outUser = outUser;
        this.inUser = inUser;
        this.rating = rating;
        this.comment = comment;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public UserDto getOutUser() {
        return outUser;
    }

    public void setOutUser(UserDto outUser) {
        this.outUser = outUser;
    }

    public UserDto getInUser() {
        return inUser;
    }

    public void setInUser(UserDto inUser) {
        this.inUser = inUser;
    }

    public int getRating() {
        return rating;
    }

    public void setRating(int rating) {
        this.rating = rating;
    }

    public String getComment() {
        return comment;
    }

    public void setComment(String comment) {
        this.comment = comment;
    }
}
