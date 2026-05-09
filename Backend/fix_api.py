import os

dir_path = "e:/CodeFolder/Github_project/BookRecProject/Backend/Api/Controllers"

def update_file(filename, replacements):
    filepath = os.path.join(dir_path, filename)
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    new_content = content
    for old, new in replacements:
        new_content = new_content.replace(old, new)
        
    if new_content != content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Updated {filename}")

update_file("BooksControllers.cs", [
    ('Route("api/[controller]")', 'Route("api/books")'),
    ('{BookId}', '{book_id}'),
    ('Guid BookId', 'Guid book_id'),
    ('(BookId', '(book_id'),
    ('{BookId}', '{book_id}')
])

update_file("RatingsController.cs", [
    ('Route("api/[controller]")', 'Route("api/ratings")'),
    ('{bookId}', '{book_id}'),
    ('Guid bookId', 'Guid book_id'),
    ('Guid userId', 'Guid user_id'),
    ('userId.ToString()', 'user_id.ToString()'),
    ('user_id = userId', 'user_id = user_id'),
    ('book_id = bookId', 'book_id = book_id')
])

update_file("ReviewsController.cs", [
    ('Route("api/[controller]")', 'Route("api/reviews")'),
    ('{bookId}', '{book_id}'),
    ('{reviewId}', '{review_id}'),
    ('Guid bookId', 'Guid book_id'),
    ('Guid reviewId', 'Guid review_id'),
    ('(bookId)', '(book_id)'),
    ('(reviewId)', '(review_id)')
])

update_file("UsersControllers.cs", [
    ('Route("api/[controller]")', 'Route("api/users")'),
    ('{UserId}', '{user_id}'),
    ('{userId}', '{user_id}'),
    ('{bookId}', '{book_id}'),
    ('Guid userId', 'Guid user_id'),
    ('Guid bookId', 'Guid book_id'),
    ('(userId)', '(user_id)'),
    ('(userId, bookId)', '(user_id, book_id)')
])
